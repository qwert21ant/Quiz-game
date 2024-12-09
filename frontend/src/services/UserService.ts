import CommonServiceBase from "./CommonServiceBase";
import UserData from "@/models/UserData";

export default class UserService extends CommonServiceBase {
  public constructor() {
    super("/user");
  }

  public async getUserData(): Promise<UserData> {
    return this.get<undefined, UserData>("");
  }
}