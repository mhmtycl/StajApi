using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Departmanlar;

public class DeleteDepartmanCommandHandler
    : ICommandHandler<DeleteDepartmanCommand, DepartmanIslemSonucu>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DeleteDepartmanCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DepartmanIslemSonucu> Handle(DeleteDepartmanCommand command)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var komut = connection.CreateCommand();

        komut.BindByName = true;

        komut.CommandText = """
            DELETE FROM DEPARTMANLAR
            WHERE DEPARTMAN_ID = :departmanId
            """;

        komut.Parameters.Add("departmanId", OracleDbType.Int32).Value =
            command.DepartmanId;

        int etkilenenSatir;

        try
        {
            etkilenenSatir = await komut.ExecuteNonQueryAsync();
        }
        catch (OracleException hata) when (hata.Number == 2292)
        {
            return new DepartmanIslemSonucu
            {
                Success = false,
                Message = "Bu departmana bağlı çalışanlar olduğu için silinemiyor."
            };
        }

        if (etkilenenSatir == 0)
        {
            return new DepartmanIslemSonucu
            {
                Success = false,
                BulunamadiMi = true,
                Message = "Silinecek departman bulunamadı."
            };
        }

        return new DepartmanIslemSonucu
        {
            Success = true,
            DepartmanId = command.DepartmanId,
            Message = "Departman silindi."
        };
    }
}
