public interface ICredentialsService {
  bool Exists(string login);

  void Add(Credentials creds);

  bool Validate(Credentials creds);
}