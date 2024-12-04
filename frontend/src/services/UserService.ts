import AuthServiceBase from "./AuthServiceBase";
import UserData from "@/models/UserData";

export default class UserService extends AuthServiceBase {
  public constructor() {
    super("/user");
  }

  public async getUserData(): Promise<UserData> {
    return this.get<undefined, UserData>("");
  }
}