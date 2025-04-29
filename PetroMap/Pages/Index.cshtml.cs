using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using PetroMap.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetroMap.Pages
{
	public class IndexModel : PageModel
	{
		private readonly IConfiguration _configuration;

		public string CoordenadasJson { get; set; }
		public string InfosJson { get; set; }

		public List<Coordenada> Coordenadas { get; set; }
		public List<Infos> Infos { get; set; }

		public IndexModel(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task OnGetAsync()
		{
			Coordenadas = await ExecutarLeituraAsync("CALL ObterCoordenadas", leitor =>
				new Coordenada
				{
					Latitude = leitor.GetString("LATITUDE_BASE_DD"),
					Longitude = leitor.GetString("LONGITUDE_BASE_DD")
				});

			Infos = await ExecutarLeituraAsync("CALL ObterInfos", leitor =>
				new Infos
				{
					Nome = leitor.GetString("POCO"),
					Cadastro = leitor.GetString("CADASTRO"),
					Estado = leitor.GetString("ESTADO"),
					Bacia = leitor.GetString("BACIA"),
					Bloco = leitor.GetString("BLOCO"),
					Campo = leitor.GetString("CAMPO"),
					Tipo = leitor.GetInt32("TIPO").ToString(),
					Categoria = leitor.GetString("CATEGORIA"),
					Reclassificacao = leitor.GetString("RECLASSIFICACAO"),
					Situacao = leitor.GetString("SITUACAO"),
					Inicio = leitor.GetString("INICIO"),
					Termino = leitor.GetString("TERMINO"),
					Conclusao = leitor.GetString("CONCLUSAO"),
					Lamina = leitor.GetInt32("LAMINA_D_AGUA_M").ToString()
				});

			CoordenadasJson = JsonSerializer.Serialize(Coordenadas);
			InfosJson = JsonSerializer.Serialize(Infos);
		}

		private async Task<List<T>> ExecutarLeituraAsync<T>(string procedure, Func<MySqlDataReader, T> map)
		{
			var lista = new List<T>();
			var connectionString = _configuration.GetConnectionString("DefaultConnection");

			await using var conexao = new MySqlConnection(connectionString);
			await conexao.OpenAsync();

			await using var comando = new MySqlCommand(procedure, conexao);
			await using var leitor = await comando.ExecuteReaderAsync();

			while (await leitor.ReadAsync())
			{
				lista.Add(map(leitor));
			}

			return lista;
		}
	}
}
