namespace Core.Database
{
    public class SqlStatement
    {
        public string Sql { get; set; }
        public object[] Params { get; set; }

        public SqlStatement(string sql, object [] paramsObjects)
        {
            Sql = sql;
            Params = paramsObjects;
        }
    }
}