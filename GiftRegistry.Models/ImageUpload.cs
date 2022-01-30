using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class ImageUpload
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public bool IsPerson { get; set; }

        public string RandomImageURL { get; private set; }


        public ImageUpload() 
        {
            RandomImageURL = "https://doodleipsum.com/500/avatar-3";
        }

        public ImageUpload(string name, byte[] image, bool isPerson)
        {
            Name = name;
            UpdateImageForBytes(image);
            IsPerson = isPerson;
            RandomImageURL = IsPerson ? "https://doodleipsum.com/500/avatar-3" : "https://doodleipsum.com/500/abstract";
        }

        public void UpdateImageForBytes(byte[] input)
        {
            var base64 = Convert.ToBase64String(input);
            Image = String.Format("data:image/gif;base64,{0}", base64);
        }
    }
}
