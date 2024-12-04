import Credentials from "@/models/Credentials";
import ServiceBase from "./ServiceBase";

export default class AuthService extends ServiceBase {
  public constructor() {
    super("/auth");
  }

  public async login(creds: Credentials): Promise<boolean> {
    try {
      await this.post("/login", creds);
      return true;
    } catch (e) {
      return false;
    }
  }

  public async signup(creds: Credentials) {
    try {
      await this.post("/signup", creds);
      return true;
    } catch (e) {
      return false;
    }
  }

  public async logout() {
    try {
      await this.post("/logout");
      return true;
    } catch (e) {
      return false;
    }
  }
}