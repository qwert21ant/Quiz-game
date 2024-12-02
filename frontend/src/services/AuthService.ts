import Credentials from "@/models/Credentials";
import ServiceBase from "./ServiceBase";

export default class AuthService extends ServiceBase {
  public constructor() {
    super("/auth");
  }

  public async login(creds: Credentials): Promise<boolean> {
    const res = await this.post("/login", creds);

    return res.status === 200;
  }

  public async signup(creds: Credentials) {
    const res = await this.post("/signup", creds);

    return res.status === 200;
  }

  public async logout() {
    const res = await this.post("/logout");

    return res.status === 200;
  }
}