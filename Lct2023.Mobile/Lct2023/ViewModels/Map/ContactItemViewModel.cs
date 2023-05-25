using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Map;

public class ContactItemViewModel : MvxNotifyPropertyChanged
{
    public ContactItemViewModel(string phone)
    {
        Phone = phone;
    }
    
    public IMvxCommand CallCommand { get; set; }
    
    public IMvxCommand CopyCommand { get; set; }
    
    public string Phone { get; }
}