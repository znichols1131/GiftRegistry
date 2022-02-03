using GiftRegistry.Data;
using GiftRegistry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Services
{
    public class ImageService
    {
        private readonly Guid _userId;

        public ImageService(Guid userId)
        {
            _userId = userId;
        }

        public bool CreateImage(ImageModel model)
        {
            var entity =
                new CustomImage()
                {
                    OwnerGUID = _userId,
                    ImageData = model.ImageData
                };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Images.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public bool CreateDefaultImage(bool isProfilePicture)
        {
            var entity =
                new CustomImage()
                {
                    OwnerGUID = _userId,
                    ImageData = GetDefaultImage(isProfilePicture)
                };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Images.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public ImageModel CreateAndReturnRandomImage(bool isProfilePicture)
        {
            if (!DeleteImagesForUser()) return null;

            if (!CreateDefaultImage(isProfilePicture)) return null;
            
            return GetLatestImageForUser();
        }

        public ImageModel GetImageByID(int id)
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var entity =
                        ctx
                            .Images
                            .Single(e => e.ImageID == id && e.OwnerGUID == _userId);

                    return
                        new ImageModel
                        {
                            ImageID = entity.ImageID,
                            OwnerGUID = entity.OwnerGUID,
                            ImageData = entity.ImageData
                        };
                }
            }catch
            {
                return null;
            }
        }

        public ImageModel GetLatestImageForUser()
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var entity =
                        ctx
                            .Images
                            .First(e => e.OwnerGUID == _userId);

                    return
                        new ImageModel
                        {
                            ImageID = entity.ImageID,
                            OwnerGUID = entity.OwnerGUID,
                            ImageData = entity.ImageData
                        };
                }
            }
            catch
            {
                return null;
            }
        }

        public ImageModel GetProfileImageForUser()
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    if (ctx.People.Any(e => e.PersonGUID == _userId))
                    {
                        Person user = ctx.People.First(e => e.PersonGUID == _userId);

                        if (user.ProfilePicture != null)
                            return new ImageModel() { ImageData = user.ProfilePicture };
                    }
                }
            }
            catch { }

            return CreateAndReturnRandomImage(true);
        }

        public bool DeleteImage(int id)
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var entity =
                        ctx
                            .Images
                            .Single(e => e.ImageID == id && e.OwnerGUID == _userId);

                    ctx.Images.Remove(entity);

                    return ctx.SaveChanges() == 1;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteImagesForUser()
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var entity =
                        ctx
                            .Images
                            .ToList();

                    foreach(var image in entity)
                    {
                        ctx.Images.Remove(image);
                    }

                    return ctx.SaveChanges() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private byte[] GetDefaultImage(string url)
        {
            // Using Doodle Ipsum for default profile pictures

            using (var webClient = new WebClient())
            {
                return webClient.DownloadData(url);
            }
        }

        private byte[] GetDefaultImage(bool isProfilePicture)
        {
            if(isProfilePicture)
            {
                return GetDefaultImage("https://doodleipsum.com/500/avatar-3");
            }else
            {
                return GetDefaultImage("https://doodleipsum.com/500/abstract");
            }
        }
    }
}
