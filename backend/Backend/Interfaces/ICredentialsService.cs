using System.Threading.Tasks;

public interface ICredentialsService {
  bool Exists(string login);

  Task Add(Credentials creds);

  bool Validate(Credentials creds);
}