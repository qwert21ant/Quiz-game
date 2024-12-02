import AuthServiceBase from "./AuthServiceBase";
import UserData from "@/models/UserData";

export default class UserService extends AuthServiceBase {
  public constructor() {
    super("/user");
  }

  public async getUserData(): Promise<UserData> {
    const res = await this.get<undefined, UserData>("");
    return res.data;
  }
}