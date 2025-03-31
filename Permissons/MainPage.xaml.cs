using System.ComponentModel.Design;
using System.Text;
using System.Threading;
using CommunityToolkit;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace Permissons
{
    public partial class MainPage : ContentPage
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        IFileSaver fileSaver;
        public MainPage(IFileSaver fileSaver)
        {
            InitializeComponent();
            this.fileSaver = fileSaver; 
        }

        private void OnCameraClicked(object sender, EventArgs e)
        {
            CheckPermissonsCamera();
        }

        private void OnGPSClicked(object sender, EventArgs e)
        {
            CheckPermissonsGPS();
        }

        private void OnSMSClicked(object sender, EventArgs e)
        {
            CheckPermissonsSMS();
        }

        private void OnPhoneClicked(object sender, EventArgs e)
        {
            CheckPermissonsPhone();
        }

        

        private async void OnSensorClicked(object sender, EventArgs e)
        {
            ToggleSensorAccelerometer();
        }

        public void ToggleSensorAccelerometer()
        {
            if (Accelerometer.Default.IsSupported)
            {
                if (!Accelerometer.Default.IsMonitoring)
                {
                    // Turn on accelerometer
                    Accelerometer.Default.ReadingChanged += Accelerometer_ReadingChanged;
                    Accelerometer.Default.Start(SensorSpeed.UI);
                }
                else
                {
                    // Turn off accelerometer
                    Accelerometer.Default.Stop();
                    Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
                    sensor.Text = "Sensor off";
                }
            }
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            // Update UI Label with accelerometer state
           
            sensor.Text = $"Accel: {e.Reading}";
        }


        private async void OnEmailClicked(object sender, EventArgs e)
        {
            if (Email.Default.IsComposeSupported)
            {

                string subject = "Hello .Net maui friends!";
                string body = "Happy Coding!";
                string[] recipients = new[] { "john@contosox.com", "jane@contosox.com" };

                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = new List<string>(recipients)
                };

                await Email.Default.ComposeAsync(message);
            }
        }

        private async void OnFileOpenClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Bitte eine Datei auswählen"
            });

            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                // Datei weiterverarbeiten
            }
        }

        private async void OnFileSaveClicked(object sender, EventArgs e)
        {
            CheckPermissonsFileSave();
        }

        private async void CheckPermissonsFileSave()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();
            }

            var status1 = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status1 != PermissionStatus.Granted)
            {
                status1 = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            if (status == PermissionStatus.Granted && status1 == PermissionStatus.Granted)
            {

                await DisplayAlert("Read and Write Permission", "Read and Write permission erlaubt!", "Ok");
                using var stream = new MemoryStream(Encoding.Default.GetBytes("Hi! .Net:Maui:Dev:Blog!"));
                // Calling  the SaveAsync method
                fileSaver.SaveAsync("SampleFile.txt", stream, cancellationTokenSource.Token);
            }
            else
            {

                await DisplayAlert("Read and Write Permission", "Read and Write permission nicht  erlaubt!", "Ok");
            }
        }


        private async void CheckPermissonsPhone()
        {

            var status = await Permissions.CheckStatusAsync<Permissions.Sms>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                await DisplayAlert("Phone", "Phone Call permission erlaubt!", "Ok");
                if (PhoneDialer.Default.IsSupported)
                {
                    PhoneDialer.Default.Open("0123456789");
                }
                else
                {
                    // Gerät unterstützt keine Telefonfunktion
                }
            }
            else
            {

                await DisplayAlert("Phone", "Phone Call permission nicht erlaubt!", "Ok");
            }

        }


        private async void CheckPermissonsSMS()
        {

            var status = await Permissions.CheckStatusAsync<Permissions.Sms>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                await DisplayAlert("SMS", "SMS permission erlaubt!", "Ok");
                var sms = new SmsMessage("Hallo, das ist eine Testnachricht", new[] { "0123456789" });

                if (Sms.Default.IsComposeSupported)
                {
                    await Sms.Default.ComposeAsync(sms);
                }
                else
                {
                    // SMS nicht verfügbar
                }
            }
            else
            {

                await DisplayAlert("SMS", "SMS permission nicht erlaubt!", "Ok");
            }
               
        }

        private async void CheckPermissonsGPS()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                // Standort abrufen
                await DisplayAlert("Standort Permission", "Standort permission erlaubt!", "Ok");
                Location location = await Geolocation.Default.GetLastKnownLocationAsync();
                if (location != null) await DisplayAlert("Standort Permission", location.Latitude + " " + location.Longitude, "Ok");
            }
            else
            {
                // Kein Zugriff auf Standort
                await DisplayAlert("Standort Permission", "Standort permission nicht  erlaubt!", "Ok");
            }
        }


        private async void CheckPermissonsCamera()
        {
            if (myCamera.IsVisible)
            {
                myCamera.IsVisible = false;
                return;
            }
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status == PermissionStatus.Granted)
            {
                await DisplayAlert("Camera Permission", "Camera permission erlaubt!", "Ok");
                // Zugriff erlaubt – Kamera öffnen
                myCamera.IsVisible = true;
               
            }
            else
            {
                await DisplayAlert("Camera Permission", "Camera permission Nicht erlaubt!", "Ok");
                // Zugriff verweigert – Benutzer informieren
            }
        }


    }

}
