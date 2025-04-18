using ServiceSendMessageWhatsapp;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace VagasApp.ConsoleCandidatura
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var respostasMapeadas = new Dictionary<string, string>
            {
                { "experiência com .net", "10 anos" },
                { "nível de inglês", "Avançado" },
                { "autorizado a trabalhar", "Sim" },
                { "visto de trabalho", "Sim" }
            };

            Console.WriteLine("🤖 Iniciando processo de candidatura simplificada no LinkedIn...");

            var options = new ChromeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("--disable-blink-features=AutomationControlled");

            var driverService = ChromeDriverService.CreateDefaultService("C:\\AllFiles\\drivers");
            driverService.HideCommandPromptWindow = false;
            driverService.Port = 9515;

            using var driver = new ChromeDriver(driverService, options);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);

            var optionsBuilder = new DbContextOptionsBuilder<VagaContext>();
            object value = optionsBuilder.UseSqlServer("Server=DELL19932607\\SQLEXPRESS;Database=VagasDB1;Trusted_Connection=True;TrustServerCertificate=True;");

            var vagaService = new VagaService(new VagaContext(optionsBuilder.Options));

            var n = (await vagaService.ObterVagasAsync()).ToList();
            var vagas = (await vagaService.ObterVagasAsync())
                //.Where(v => v.CandidaturaSimplificada == "Sim" && v.Aplicada == false)
                .Where(v => v.CandidaturaSimplificada == "Sim" || v.CandidaturaSimplificada == "Nao")
                .ToList();
            vagas = (await vagaService.ObterVagasAsync()).ToList();

            foreach (var vaga in vagas)
            {
                Console.WriteLine($"\n➡️ Candidatando-se: {vaga.Titulo} | {vaga.Empresa}");
                var perguntasNaoMapeadas = new List<string>();

                try
                {
                    driver.Navigate().GoToUrl(vaga.Link);
                    Thread.Sleep(3000);

                    var botaoCandidatar = driver.FindElements(By.XPath("//button[contains(.,'Candidatar-se')]"))
                                                .FirstOrDefault();

                    if (botaoCandidatar != null)
                    {
                        botaoCandidatar.Click();
                        Thread.Sleep(2000);

                        var campos = driver.FindElements(By.CssSelector("form label"));

                        foreach (var campo in campos)
                        {
                            var textoLabel = campo.Text.ToLower().Trim();
                            var match = respostasMapeadas.FirstOrDefault(p => textoLabel.Contains(p.Key.ToLower()));

                            if (!string.IsNullOrEmpty(match.Key))
                            {
                                try
                                {
                                    var input = campo.FindElement(By.XPath("following-sibling::*[1]"));
                                    if (input.TagName == "input" || input.TagName == "textarea")
                                        input.SendKeys(match.Value);
                                    else if (input.TagName == "select")
                                    {
                                        var selectElement = new SelectElement(input);
                                        selectElement.SelectByText(match.Value);
                                    }
                                }
                                catch (Exception exCampo)
                                {
                                    Console.WriteLine($"⚠️ Erro ao preencher campo: {textoLabel} - {exCampo.Message}");
                                }
                            }
                            else
                            {
                                perguntasNaoMapeadas.Add(textoLabel);
                            }
                        }

                        var botaoEnviar = driver.FindElements(By.XPath("//button[contains(.,'Enviar') or contains(.,'Avançar')]"))
                                                 .FirstOrDefault();

                        botaoEnviar?.Click();

                        vaga.Aplicada = true;
                        await vagaService.AtualizarVagaAsync(vaga);
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Botão de candidatura não encontrado.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erro ao processar vaga: {ex.Message}");
                }

                await vagaService.GravarPerguntasPendentesAsync(perguntasNaoMapeadas, vaga.Link);
            }
        }
    }
}
















//using ServiceSendMessageWhatsapp;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Chrome;
//using OpenQA.Selenium.Support.UI;

//namespace VagasApp.ConsoleCandidatura
//{
//    class Program
//    {
//        static async Task Main(string[] args)
//        {
//            var respostasMapeadas = new Dictionary<string, string>
//            {
//                { "experiência com .net", "10 anos" },
//                { "nível de inglês", "Avançado" },
//                { "autorizado a trabalhar", "Sim" },
//                { "visto de trabalho", "Sim" }
//            };

//            Console.WriteLine("🤖 Iniciando processo de candidatura simplificada no LinkedIn...");

//            var options = new ChromeOptions();
//            options.AddArgument("start-maximized");
//            options.AddArgument("--disable-blink-features=AutomationControlled");

//            var driverService = ChromeDriverService.CreateDefaultService("C:\\AllFiles\\drivers");
//            driverService.HideCommandPromptWindow = false;
//            driverService.Port = 9515;

//            using var driver = new ChromeDriver(driverService, options);
//            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);

//            var vagaService = new VagaService(new VagaContext());
//            var vagas = (await vagaService.ObterVagasAsync())
//                .Where(v => v.CandidaturaSimplificada == "Sim" && v.Aplicada == false)
//                .ToList();

//            foreach (var vaga in vagas)
//            {
//                Console.WriteLine($"\n➡️ Candidatando-se: {vaga.Titulo} | {vaga.Empresa}");
//                var perguntasNaoMapeadas = new List<string>();

//                try
//                {
//                    driver.Navigate().GoToUrl(vaga.Link);
//                    Thread.Sleep(3000);

//                    var botaoCandidatar = driver.FindElements(By.XPath("//button[contains(.,'Candidatar-se')]"))
//                                                .FirstOrDefault();

//                    if (botaoCandidatar != null)
//                    {
//                        botaoCandidatar.Click();
//                        Thread.Sleep(2000);

//                        var campos = driver.FindElements(By.CssSelector("form label"));

//                        foreach (var campo in campos)
//                        {
//                            var textoLabel = campo.Text.ToLower().Trim();
//                            var match = respostasMapeadas.FirstOrDefault(p => textoLabel.Contains(p.Key.ToLower()));

//                            if (!string.IsNullOrEmpty(match.Key))
//                            {
//                                try
//                                {
//                                    var input = campo.FindElement(By.XPath("following-sibling::*[1]"));
//                                    if (input.TagName == "input" || input.TagName == "textarea")
//                                        input.SendKeys(match.Value);
//                                    else if (input.TagName == "select")
//                                    {
//                                        var selectElement = new SelectElement(input);
//                                        selectElement.SelectByText(match.Value);
//                                    }
//                                }
//                                catch (Exception exCampo)
//                                {
//                                    Console.WriteLine($"⚠️ Erro ao preencher campo: {textoLabel} - {exCampo.Message}");
//                                }
//                            }
//                            else
//                            {
//                                perguntasNaoMapeadas.Add(textoLabel);
//                            }
//                        }

//                        var botaoEnviar = driver.FindElements(By.XPath("//button[contains(.,'Enviar') or contains(.,'Avançar')]"))
//                                                 .FirstOrDefault();

//                        botaoEnviar?.Click();

//                        vaga.Aplicada = true;
//                        await vagaService.AtualizarVagaAsync(vaga);
//                    }
//                    else
//                    {
//                        Console.WriteLine("⚠️ Botão de candidatura não encontrado.");
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"❌ Erro ao processar vaga: {ex.Message}");
//                }

//                await vagaService.GravarPerguntasPendentesAsync(perguntasNaoMapeadas, vaga.Link);
//            }
//        }
//    }
//}
