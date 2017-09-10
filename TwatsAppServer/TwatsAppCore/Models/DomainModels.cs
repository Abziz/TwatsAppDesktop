using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwatsAppCore.Models
{
    public class TwatsAppUser 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Index(IsUnique =true)]
        [StringLength(64)]
        public string UserName { get; set; }

        [Required]
        [StringLength(256)]//for SHA 256
        public string PasswordHash { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTimeOffset TimeCreated { get; set; } = DateTimeOffset.Now;

    }

    public class Conversation
    {
        [Key]
        public int Id { get; set; }    

        public virtual Message LastMessage { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }

    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public virtual TwatsAppUser From { get; set; }

        [Required]
        public virtual TwatsAppUser To { get; set; }

        [Required]
        public DateTimeOffset DispatchedAt { get; set; } = DateTimeOffset.Now;

        [Required]
        public bool SeenBySender { get; set; } = false;

        [Required]
        public bool SeenByReceiver { get; set; } = false;

        [Required]
        [StringLength(512,MinimumLength =1)]
        public string Content { get; set; }
    }
}
