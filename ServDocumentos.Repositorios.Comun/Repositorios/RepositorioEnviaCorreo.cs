using cmn.std.Log;
using Newtonsoft.Json;
using ServDocumentos.Core.Contratos.Repositorios.Comun;
using ServDocumentos.Core.Dtos.Comun.Correo;
using System;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServDocumentos.Repositorios.Comun.Repositorios
{
    public class RepositorioEnviaCorreo : RepositorioBase, IRepositorioEnviaCorreo
    {
        public RepositorioEnviaCorreo(IDbConnection connection, Func<IDbTransaction> transaction, GestorLog gestorLog) : base(connection, transaction, gestorLog)
        {
        }
       /* public EstatusCorreoDto Enviar(CorreoDto c)
        {
            var messageToSend = new MimeMessage();
            messageToSend.From.Add(InternetAddress.Parse(Constantes.CorreoFrom));
            messageToSend.Subject = c.Asunto;
            if (c.Correo != null)
            {
                messageToSend.To.Add(InternetAddress.Parse(c.Correo));
            }
            if (c.CC != null)
            {
                InternetAddressList list = new InternetAddressList();
                foreach (string cc in c.CC)
                {
                    list.Add(InternetAddress.Parse(cc));
                }
                messageToSend.Cc.AddRange(list);
            }
            var builder = new BodyBuilder();
            builder.TextBody = c.Cuerpo;
            if (c.Adjuntos != null)
            {
                foreach (var cc in c.Adjuntos)
                {
                    builder.Attachments.Add(cc.Nombre, cc.Binario);
                }
            }


            messageToSend.Body = builder.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {

                    smtp.Connect(Constantes.SMTPHost, Constantes.SMTPPort);
                    smtp.Authenticate(Constantes.SMTPUser, Constantes.SMTPPassword);
                    smtp.Send(messageToSend);
                    smtp.Disconnect(true);
                    return new EstatusCorreoDto { Observaciones = "", Estatus = "Enviado" };
                }
                catch (Exception err) { return new EstatusCorreoDto { Observaciones = err.Message, Estatus = "ErrorEnviado" }; }

            }
        }
        public async Task<EstatusCorreoDto> EnviarAsync(CorreoDto c)
        {
            var messageToSend = new MimeMessage();
            messageToSend.From.Add(InternetAddress.Parse(Constantes.CorreoFrom));
            messageToSend.Subject = c.Asunto;
            if (c.Correo != null)
            {
                messageToSend.To.Add(InternetAddress.Parse(c.Correo));
            }
            if (c.CC != null)
            {
                InternetAddressList list = new InternetAddressList();
                foreach (string cc in c.CC)
                {
                    list.Add(InternetAddress.Parse(cc));
                }
                messageToSend.Cc.AddRange(list);
            }
            var builder = new BodyBuilder();
            builder.TextBody = c.Cuerpo;
            if (c.Adjuntos != null)
            {
                foreach (var cc in c.Adjuntos)
                {
                    builder.Attachments.Add(cc.Nombre, cc.Binario);
                }
            }


            messageToSend.Body = builder.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {

                    await smtp.ConnectAsync(Constantes.SMTPHost, Constantes.SMTPPort);
                    await smtp.AuthenticateAsync(Constantes.SMTPUser, Constantes.SMTPPassword);
                    await smtp.SendAsync(messageToSend);
                    await smtp.DisconnectAsync(true);
                    return new EstatusCorreoDto { Observaciones = "", Estatus = "Enviado", FechaEnvio = DateTime.Now };
                }
                catch (Exception err) { return new EstatusCorreoDto { Observaciones = err.Message, Estatus = "ErrorEnviado", FechaEnvio = null }; }

            }
        }
        public async Task<EstatusCorreoDto> EnviarSenGridAsync(CorreoSengrid mail)
        {
            try
            {
                var client = new SendGridClient(mail.ApiKey);
                var from = new EmailAddress(mail.Remitente);
                var subject = mail.Asunto;
                
                var to = new EmailAddress(mail.Destinatario);
                var body ="";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, body, "");
                msg.HtmlContent = mail.Contenido;
                if (mail.DestinatariosCC != null)
                {
                    foreach (var item in mail.DestinatariosCC)
                    {
                        msg.AddCc(item.Email);
                    }
                }
                if (mail.Adjuntos != null)
                {
                    foreach (var item in mail.Adjuntos)
                    {
                        msg.AddAttachment(item.Nombre, item.Contenido);

                    }
                }
                var response = await client.SendEmailAsync(msg);

                return new EstatusCorreoDto { Observaciones = "", Estatus = "Enviado", FechaEnvio = DateTime.Now };


            }
            catch (Exception err)
            {
                return new EstatusCorreoDto { Observaciones = err.Message, Estatus = "ErrorEnviado", FechaEnvio = null };
            }
        }
        */
        public async Task<EstatusCorreoDto> EnviarSenGridPostAsync(CorreoSengridPost mail, string url)
        {
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(mail), Encoding.UTF8, "application/json");

                using (var httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                        (message, cert, chain, errors) => true;
                    using (var httpClient = new HttpClient(httpClientHandler))
                    {
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        var apiResponse = await httpClient.PostAsync(url, content);
                        await apiResponse.Content.ReadAsStringAsync();
                        if (apiResponse.IsSuccessStatusCode)
                            return new EstatusCorreoDto { Observaciones = "", Estatus = "Enviado", FechaEnvio = DateTime.Now };
                        else
                            return new EstatusCorreoDto { Observaciones = apiResponse.StatusCode.ToString()+ ": "+ apiResponse.RequestMessage.ToString(), Estatus = "ErrorEnviado", FechaEnvio = null };

                    }
                }

            }
            catch (Exception err)
            {
                return new EstatusCorreoDto { Observaciones = err.Message, Estatus = "ErrorEnviado", FechaEnvio = null };
            }
        }
    }
}
