using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using VagasApp.Models;
using VagasApp.Data;
using System.Text;
using Realocations.Api.Model;

namespace Realocations.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly VagaContext _context;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, VagaContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("BuscarVagasLinkedin")]
        public async Task<ActionResult<List<string>>> BuscarVagasLinkedin(
            [FromQuery] List<string> palavrasChave,
            [FromQuery] List<string> paises,
            [FromQuery] int horas = 1)
        {
            if (horas <= 0 || horas > 24)
                return BadRequest("O parâmetro 'horas' deve estar entre 1 e 24.");

            var logs = new List<string>();

            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("start-maximized");
            options.AddArgument("user-agent=Mozilla/5.0");

            var driverService = ChromeDriverService.CreateDefaultService("C:\\driverss");
            driverService.HideCommandPromptWindow = true;
            driverService.Port = 9515;

            using var driver = new ChromeDriver(driverService, options);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);

            foreach (var palavra in palavrasChave)
            {
                foreach (var pais in paises)
                {
                    string url = $"https://www.linkedin.com/jobs/search/?keywords={Uri.EscapeDataString(palavra)}&location={Uri.EscapeDataString(pais)}&f_TPR=r{horas * 3600}&f_WT=2%2C3";

                    try
                    {
                        driver.Navigate().GoToUrl(url);
                        Thread.Sleep(2000);

                        var elementos = driver.FindElements(By.CssSelector("ul.jobs-search__results-list li"));

                        foreach (var vaga in elementos)
                        {
                            try
                            {
                                var titulo = vaga.FindElement(By.CssSelector(".base-search-card__title")).Text;
                                var empresa = vaga.FindElement(By.CssSelector(".base-search-card__subtitle")).Text;
                                var local = vaga.FindElement(By.CssSelector(".job-search-card__location")).Text;
                                var dataTexto = vaga.FindElement(By.CssSelector("time")).GetAttribute("datetime");
                                var link = vaga.FindElement(By.CssSelector("a.base-card__full-link")).GetAttribute("href");

                                string candidaturaSimplificada = vaga.Text.ToLower().Contains("candidatura simplificada") ? "Sim" : "Não";

                                // 📍 Verifica modelo da vaga
                                string modelo = "Presencial";
                                var localLower = local.ToLower();
                                if (localLower.Contains("remoto") || localLower.Contains("remote"))
                                    modelo = "Remoto";
                                else if (localLower.Contains("híbrido") || localLower.Contains("hybrid"))
                                    modelo = "Híbrido";

                                //// ❌ Ignora vagas presenciais
                                //if (modelo == "Presencial")
                                //    continue;

                                // 🗓️ Parse da data
                                if (!DateTime.TryParse(dataTexto, out var data))
                                    data = DateTime.Now;

                                //// 🔁 Evita duplicatas
                                //bool jaExiste = await _context.Vagas.AnyAsync(v => v.Link == link);
                                //if (jaExiste) continue;

                                var vagaDb = new Vaga
                                {
                                    Titulo = titulo,
                                    Empresa = empresa,
                                    Local = local,
                                    Modelo = modelo,
                                    CandidaturaSimplificada = candidaturaSimplificada,
                                    Data = data,
                                    Link = link,
                                    Aplicada = false
                                };

                                _context.Vagas.Add(vagaDb);
                                logs.Add($"✅ {titulo} | {empresa} | {local}");
                            }
                            catch (Exception exItem)
                            {
                                _logger.LogWarning("Erro ao processar item: {Message}", exItem.Message);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao buscar vagas para {Palavra} em {Pais}", palavra, pais);
                    }
                }
            }

            return Ok(logs);
        }



        // WeatherForecastController.cs (somente busca e grava vagas)
        [HttpGet("BuscarVagasLinkedinMetodoNovo")]
        public async Task<ActionResult<List<string>>> BuscarVagasLinkedinMetodoNovo(
            [FromQuery] List<string> palavrasChave,
            [FromQuery] List<string> paises,
            [FromQuery] int horas = 24)
        {
            if (horas <= 0 || horas > 24)
                return BadRequest("O parâmetro 'horas' deve estar entre 1 e 24.");

            var logs = new List<string>();
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("start-maximized");
            options.AddArgument("user-agent=Mozilla/5.0");

            var driverService = ChromeDriverService.CreateDefaultService("C:\\driverss");
            driverService.HideCommandPromptWindow = true;
            driverService.Port = 9515;

            using var driver = new ChromeDriver(driverService, options);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);

            foreach (var palavra in palavrasChave)
            {
                foreach (var pais in paises)
                {
                    string url = $"https://www.linkedin.com/jobs/search/?keywords={Uri.EscapeDataString(palavra)}&location={Uri.EscapeDataString(pais)}&f_TPR=r{horas * 3600}&f_WT=2%2C3";

                    try
                    {
                        driver.Navigate().GoToUrl(url);
                        Thread.Sleep(3000);

                        var elementos = driver.FindElements(By.CssSelector("ul.jobs-search__results-list li"));

                        foreach (var vaga in elementos)
                        {
                            try
                            {
                                var titulo = vaga.FindElement(By.CssSelector(".base-search-card__title")).Text;
                                var empresa = vaga.FindElement(By.CssSelector(".base-search-card__subtitle")).Text;
                                var local = vaga.FindElement(By.CssSelector(".job-search-card__location")).Text;
                                var dataTexto = vaga.FindElement(By.CssSelector("time")).GetAttribute("datetime");
                                var link = vaga.FindElement(By.CssSelector("a.base-card__full-link")).GetAttribute("href");

                                string candidaturaSimplificada = vaga.Text.ToLower().Contains("candidatura simplificada") ? "Sim" : "Não";

                                string modelo = "Presencial";
                                var localLower = local.ToLower();
                                if (localLower.Contains("remoto") || localLower.Contains("remote"))
                                    modelo = "Remoto";
                                else if (localLower.Contains("híbrido") || localLower.Contains("hybrid"))
                                    modelo = "Híbrido";

                                if (!DateTime.TryParse(dataTexto, out var data))
                                    data = DateTime.Now;

                                bool jaExiste = await _context.Vagas.AnyAsync(v => v.Link == link);
                                if (jaExiste) continue;

                                _context.Vagas.Add(new Vaga
                                {
                                    Titulo = titulo,
                                    Empresa = empresa,
                                    Local = local,
                                    Modelo = modelo,
                                    CandidaturaSimplificada = candidaturaSimplificada,
                                    Data = data,
                                    Link = link,
                                    Aplicada = false
                                });

                                logs.Add($"✅ {titulo} | {empresa} | {local}");
                            }
                            catch (Exception exItem)
                            {
                                _logger.LogWarning("Erro ao processar item: {Message}", exItem.Message);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao buscar vagas para {Palavra} em {Pais}", palavra, pais);
                    }
                }
            }

            return Ok(logs);
        }





        [HttpGet("BuscarVagasLinkedinEfetivar")]
        public async Task<ActionResult<List<string>>> BuscarVagasLinkedinEfetivar(
 [FromQuery] List<string> palavrasChave,
 [FromQuery] List<string> paises,
 [FromQuery] int horas = 24)
        {
            if (horas <= 0 || horas > 24)
                return BadRequest("O parâmetro 'horas' deve estar entre 1 e 24.");

            var logs = new List<string>();

            var respostasMapeadas = new Dictionary<string, string>
    {
        { "experiência com .net", "10 anos" },
        { "nível de inglês", "Avançado" },
        { "autorizado a trabalhar", "Sim" },
        { "visto de trabalho", "Sim" }
    };

            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("start-maximized");
            options.AddArgument("user-agent=Mozilla/5.0");

            var driverService = ChromeDriverService.CreateDefaultService("C:\\driverss");
            driverService.HideCommandPromptWindow = true;
            driverService.Port = 9515;

            using var driver = new ChromeDriver(driverService, options);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);

            foreach (var palavra in palavrasChave)
            {
                foreach (var pais in paises)
                {
                    string url = $"https://www.linkedin.com/jobs/search/?keywords={Uri.EscapeDataString(palavra)}&location={Uri.EscapeDataString(pais)}&f_TPR=r{horas * 3600}&f_WT=2%2C3";

                    try
                    {
                        driver.Navigate().GoToUrl(url);
                        Thread.Sleep(2000);

                        var elementos = driver.FindElements(By.CssSelector("ul.jobs-search__results-list li"));

                        foreach (var vaga in elementos)
                        {
                            try
                            {
                                var titulo = vaga.FindElement(By.CssSelector(".base-search-card__title")).Text;
                                var empresa = vaga.FindElement(By.CssSelector(".base-search-card__subtitle")).Text;
                                var local = vaga.FindElement(By.CssSelector(".job-search-card__location")).Text;
                                var dataTexto = vaga.FindElement(By.CssSelector("time")).GetAttribute("datetime");
                                var link = vaga.FindElement(By.CssSelector("a.base-card__full-link")).GetAttribute("href");

                                string candidaturaSimplificada = vaga.Text.ToLower().Contains("candidatura simplificada") ? "Sim" : "Não";

                                string modelo = "Presencial";
                                var localLower = local.ToLower();
                                if (localLower.Contains("remoto") || localLower.Contains("remote"))
                                    modelo = "Remoto";
                                else if (localLower.Contains("híbrido") || localLower.Contains("hybrid"))
                                    modelo = "Híbrido";


                                if (!DateTime.TryParse(dataTexto, out var data))
                                    data = DateTime.Now;


                                // Acessar a vaga e tentar se candidatar
                                var perguntasNaoMapeadas = new List<string>();

                                driver.Navigate().GoToUrl(link);
                                Thread.Sleep(2000);

                                var botaoCandidatar = driver.FindElements(By.XPath("//button[contains(.,'Candidatar-se')]")).FirstOrDefault();

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
                                                    var selectElement = new OpenQA.Selenium.Support.UI.SelectElement(input);
                                                    selectElement.SelectByText(match.Value);
                                                }
                                            }
                                            catch (Exception exCampo)
                                            {
                                                _logger.LogWarning("Erro ao preencher campo '{Texto}': {Erro}", textoLabel, exCampo.Message);
                                            }
                                        }
                                        else
                                        {
                                            perguntasNaoMapeadas.Add(textoLabel);
                                        }
                                    }

                                    // Salva perguntas não mapeadas
                                    foreach (var pergunta in perguntasNaoMapeadas.Distinct())
                                    {
                                        if (!await _context.PerguntaPendentes.AnyAsync(p => p.Texto == pergunta))
                                        {
                                            _context.PerguntaPendentes.Add(new PerguntaPendente
                                            {
                                                Texto = pergunta,
                                                OrigemVaga = link
                                            });
                                        }
                                    }

                                    var botaoProximo = driver.FindElements(By.XPath("//button[contains(.,'Avançar') or contains(.,'Enviar')]")).FirstOrDefault();
                                    botaoProximo?.Click();
                                }

                                _context.Vagas.Add(new Vaga
                                {
                                    Titulo = titulo,
                                    Empresa = empresa,
                                    Local = local,
                                    Modelo = modelo,
                                    CandidaturaSimplificada = candidaturaSimplificada,
                                    Data = data,
                                    Link = link,
                                    Aplicada = false
                                });

                                logs.Add($"✅ {titulo} | {empresa} | {local}");
                            }
                            catch (Exception exItem)
                            {
                                _logger.LogWarning("Erro ao processar item: {Message}", exItem.Message);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao buscar vagas para {Palavra} em {Pais}", palavra, pais);
                    }
                }
            }

            return Ok(logs);
        }



        [HttpDelete("LimparDados")]
        public async Task<IActionResult> LimparDados()
        {
            try
            {
                _context.Vagas.RemoveRange(_context.Vagas);
                _context.PerguntaPendentes.RemoveRange(_context.PerguntaPendentes);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Todas as vagas e perguntas pendentes foram removidas com sucesso.");

                return Ok("Dados removidos com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar limpar os dados do banco.");
                return StatusCode(500, "Erro interno ao tentar limpar os dados.");
            }
        }
    }




}









