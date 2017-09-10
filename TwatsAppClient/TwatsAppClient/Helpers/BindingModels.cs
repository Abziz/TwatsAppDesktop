using MyToolkit.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
/// <summary>
/// Just some binding models and data transfer objects some of them implement the InotifyPropertyChanged
/// </summary>
namespace TwatsAppClient.Helpers
{
    public class AccountBindingModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class SendMessageBindingModel
    {

        public int From { get; set; }
        public int To { get; set; }
        public string Content { get; set; }
        public DateTimeOffset DispatchedAt { get; set; } = DateTimeOffset.Now;
    }

    public class ContactDto : INotifyPropertyChanged
    {
        private UserDto user;
        private MessageDto lastMessage;
        private ObservableCollectionView<MessageDto> messages;

        public UserDto User {
            get { return user; }
            set {
                user = value;
                NotifyPropertyChanged();
            }
        }
        public MessageDto LastMessage {
            get { return lastMessage; }
            set {
                lastMessage = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollectionView<MessageDto> Messages {
            get { return messages; }
            set {
                messages = value;
                NotifyPropertyChanged();
            }
        }
       

        public ContactDto()
        {
            Messages = new ObservableCollectionView<MessageDto>()
            {
                Order = p => p.DispatchedAt
            };
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }



    public class UserDto : INotifyPropertyChanged
    {
        private int id = 0;
        private string fullName = string.Empty;
        private DateTimeOffset joined;

        public int Id {
            get { return id; }
            set {
                id = value;
                NotifyPropertyChanged();
            }
        }
        public string FullName {
            get { return fullName; }
            set {
                fullName = value;
                NotifyPropertyChanged();
            }
        }

        public DateTimeOffset Joined {
            get { return joined; }
            set {
                joined = value;
                NotifyPropertyChanged();
            }
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class MessageDto : INotifyPropertyChanged
    {
        private UserDto from;
        private UserDto to;
        private string content;
        private DateTimeOffset dispatchAt;

        public UserDto From {
            get { return from; }
            set {
                from = value;
                NotifyPropertyChanged();
            }
        }

        public UserDto To {
            get { return to; }
            set {
                to = value;
                NotifyPropertyChanged();
            }
        }

        public string Content {
            get { return content; }
            set {
                content = value;
                NotifyPropertyChanged();
            }
        }

        public DateTimeOffset DispatchedAt {
            get { return dispatchAt; }
            set {
                dispatchAt = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }



}
