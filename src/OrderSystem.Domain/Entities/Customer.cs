namespace OrderSystem.Domain.Entities;
using OrderSystem.Domain.Common;
using System.Net.Mail;

public class Customer : BaseEntity
{
    public string Name { get; private set; }
    public string Email { get; private set; }

    public Customer(string name, string email)
    {
        if (name is null)
        {
            throw new ArgumentNullException(nameof(name), "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (email is null)
        {
            throw new ArgumentNullException(nameof(email), "Email is required.");
        }

        if (!IsValidEmail(email))
        {
            throw new ArgumentException("Invalid email address.", nameof(email));
        }

        Name = name;
        Email = email;
    }

    public void UpdateEmail(string newEmail)
    {
        if (newEmail is null)
        {
            throw new ArgumentNullException(nameof(newEmail), "Email is required.");
        }

        if (!IsValidEmail(newEmail))
        {
            throw new ArgumentException("Invalid email address.", nameof(newEmail));
        }

        Email = newEmail;
        UpdateTimestamp();
    }

    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) 
            && MailAddress.TryCreate(email, out _);
    }
}