using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhotoApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            btnSaveToAlbum.Clicked += BtnSaveToAlbum_Clicked;
            btnDontSaveToAlbum.Clicked += BtnDontSaveToAlbum_Clicked;
        }
        private async void BtnSaveToAlbum_Clicked(object sender, EventArgs e)
        {
            await TakePicture(true);
        }

        private async void BtnDontSaveToAlbum_Clicked(object sender, EventArgs e)
        {
            await TakePicture(false);
        }

        private async Task TakePicture(bool saveToAlbum)
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
            }

            if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }


                var name = string.Format("PhotoApp_{0}", DateTime.Now.ToString("yyMMddhhmmss"));

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Rear,
                    SaveToAlbum = saveToAlbum,
                    Name = name,

                });

                if (file == null)
                    return;

                //await DisplayAlert("File Location", file.Path, "OK");

                image.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }
            else
            {
                await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
                //On iOS you may want to send your user to the settings screen.
                //CrossPermissions.Current.OpenAppSettings();
            }
        }
    }
}

