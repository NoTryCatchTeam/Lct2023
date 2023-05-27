using System;
using System.Linq;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Auth;

public class SignUpFields : MvxNotifyPropertyChanged
{
    private string _email;
    private string _username;
    private DateTimeOffset? _birthday;

    public string Email
    {
        get => _email;
        set
        {
            if (!SetProperty(ref _email, value))
            {
                return;
            }

            Username = _email.Split("@").FirstOrDefault();
        }
    }

    public string Username
    {
        get => _username;
        private set => SetProperty(ref _username, value);
    }

    public string Password { get; set; }

    public string RepeatPassword { get; set; }

    public string PhotoBase64 { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public DateTimeOffset? Birthday
    {
        get => _birthday;
        set => SetProperty(ref _birthday, value);
    }
}