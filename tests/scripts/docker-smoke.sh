set -euo pipefail

# Cleanup method, downs disposable stack
cleanup() {
    docker compose -p enterprise-smoke down -v
}

# Restore method, ups normal stack
restore_normal_stack() {
    if [ "${CI:-}" != "true" ]; then
        docker compose up --build -d
    fi
}

# On exit trap, print ps and log info, run cleanup, restore normal stack
on_exit() {
    status=$?
    if [ "$status" -ne 0 ]; then
        echo "Smoke test failed with exit code $status"
        docker compose -p enterprise-smoke ps || true
        docker compose -p enterprise-smoke logs --tail=100 db migrate api || true
        cleanup || true
        restore_normal_stack || true
    fi
}
trap on_exit EXIT

# Create resource method, call API to create specified resource
create_resource() {
    local resource_name="$1"
    local endpoint="$2"
    local payload="$3"
    local body
    local status
    local id

    body=$(mktemp)
    status=$(
        curl --fail-with-body --silent --show-error \
            --output "$body" --write-out "%{http_code}" \
            -H "Content-Type: application/json" \
            -d "$payload" \
            "$endpoint"
    ) || {
        rm -f "$body"
        return 1
    }

    if [ "$status" != "201" ]; then
        echo "$resource_name creation returned HTTP $status" >&2
        cat "$body" >&2
        rm -f "$body"
        return 1
    fi

    id=$(sed -n 's/.*"id":"\([^"]*\)".*/\1/p' "$body")
    rm -f "$body"

    if [ -z "$id" ]; then
        echo "$resource_name response did not contain an id" >&2
        return 1
    fi

    echo "$resource_name created: $id" >&2
    printf '%s' "$id"
}

# Down current stack
docker compose down

# Start disposable stack
docker compose -p enterprise-smoke up --build -d

# API health check
ready=false
for _ in $(seq 1 60); do
    if curl --fail --silent --output /dev/null \
        "http://localhost:8080/health"; then
        ready=true
        break
    fi
    sleep 1
done

# See logs if API not ready
if [ "$ready" != true ]; then
    docker compose -p enterprise-smoke ps
    docker compose -p enterprise-smoke logs --tail=100 db migrate api
    echo "API could not be reached"
    exit 1
fi

# Find migrate container
migrate_id="$(docker compose -p enterprise-smoke ps -a -q migrate)"

# Print error if migrate container does not exist
if [ -z "$migrate_id" ]; then
    echo "Migrate container not found"
    exit 1
fi

# Inspect migrate container
migrate_status="$(docker inspect "$migrate_id" --format '{{.State.Status}}')"
migrate_exit_code="$(docker inspect "$migrate_id" --format '{{.State.ExitCode}}')"
echo "Migrator: status=$migrate_status exit-code=$migrate_exit_code"
docker compose -p enterprise-smoke logs migrate

# Exit if exit code of migrate container is not 0 and see log
if [ "$migrate_exit_code" != "0" ]; then
    exit 1
fi

# Create customer
customer_id=$(
    create_resource "Customer" \
        "http://localhost:8080/api/customers" \
        '{"name":"Smoke Customer","email":"smoke@example.com"}'
) || exit 1

# Create product
product_id=$(
    create_resource "Product" \
        "http://localhost:8080/api/products" \
        '{"name":"Smoke Product","currentPrice":12.50}'
) || exit 1

# Create order
order_id=$(
    create_resource "Order" \
        "http://localhost:8080/api/orders" \
        "{\"customerId\":\"$customer_id\",\"orderItems\":[{\"productId\":\"$product_id\",\"quantity\":2}]}"
) || exit 1

# Inspect ps and logs
docker compose -p enterprise-smoke ps
docker compose -p enterprise-smoke logs --tail=100 db migrate api

# Cleanup disposable stack, disable trap, start main stack
docker compose -p enterprise-smoke down -v
trap - EXIT
restore_normal_stack