// See https://aka.ms/new-console-template for more information
using AmigoSecreto.Models;
using AmigoSecreto.Utility;
using System.Net.Mail;
using System.Net.Mime;

Console.WriteLine("Hello, World!");
Console.WriteLine("It is that time of year again");
Console.WriteLine("Sso the deed must be done");
Console.WriteLine();

var participantes = new List<Amigo>();
var control = false;

while (control || participantes.Count < 3)
{
    var participante = new Amigo();
    Console.WriteLine("Enter the participant's name:");
    participante.Nome = Console.ReadLine();
    Console.WriteLine("Enter the participant's email:");
    participante.Email = Console.ReadLine();

    participantes.Add(participante);

    if (participantes.Count >= 3)
    {
        var secondControl = false;
        do
        {
            Console.WriteLine("Do you wish to add another participant? (Y/n)");
            var answer = Console.ReadLine();

            (control, secondControl) = answer switch
            {
                "y" => (true, false),
                "Y" => (true, false),
                "n" => (false, false),
                "N" => (false, false),
                "" => (true, false),
                _ => (false, true)
            };
        } while (secondControl);
        
    }
}

participantes.ForEach(p => Console.WriteLine($"{p.Nome} - {p.Email}"));

Random randNumber = new Random();

var auxNumber = randNumber.Next(0, participantes.Count - 2);

for(var i = 0; i < participantes.Count; i++)
{
    if (i == participantes.Count - 1 && participantes.All(p => p.AmigoSecreto != participantes[i].Nome))
    {   
        participantes[i].AmigoSecreto = participantes[auxNumber].AmigoSecreto;
        participantes[auxNumber].AmigoSecreto = participantes[i].Nome;
    }
    else
    {
        var nextAmigo = participantes[randNumber.Next(0, participantes.Count - 1)];

        while (participantes.Any(p => p.AmigoSecreto == nextAmigo.Nome) || nextAmigo.Nome == participantes[i].Nome)
        {
            nextAmigo = participantes[randNumber.Next(0, participantes.Count - 1)];
        }
        participantes[i].AmigoSecreto = nextAmigo.Nome;
    }
}

for (int i = 0; i < participantes.Count; i++)
{
    Console.WriteLine($"Participante {i} pegou participante {participantes.FindIndex(p => p.Nome == participantes[i].AmigoSecreto)}");
}

foreach (var pessoa in participantes)
{
    MailMessage newMail = new();

    SmtpClient client = new("smtp.office365.com");

    // Follow the RFS 5321 Email Standard
    newMail.From = new MailAddress("", "Amigo Secreto da Família");

    newMail.To.Add(pessoa.Email);// declare the email subject

    newMail.Subject = $"{pessoa.Nome}, seu amigo secreto é..."; // use HTML for the email body

    newMail.IsBodyHtml = true;

    string htmlBody = $"<p>---------------------------------------------------------------------------------------------------</p><br/><p>~~~~~</p><br/><h1>{pessoa.AmigoSecreto}</h1>";
    newMail.Body = htmlBody;

    // enable SSL for encryption across channels
    client.EnableSsl = true;
    // Port 465 for SSL communication
    client.Port = 587;
    client.UseDefaultCredentials = false;
    // Provide authentication information with Gmail SMTP server to authenticate your sender account
    client.Credentials = new System.Net.NetworkCredential("", "");

    client.Send(newMail); // Send the constructed mail
}