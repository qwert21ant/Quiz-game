import RoomConfig from "@/models/RoomConfig";
import AuthServiceBase from "./AuthServiceBase";

export default class RoomService extends AuthServiceBase {
  public constructor() {
    super("/room");
  }

  public async getConfig(): Promise<RoomConfig> {
    return (await this.get<undefined, RoomConfig>("/config")).data;
  }

  public async updateConfig(roomConfig: RoomConfig): Promise<void> {
    await this.post<RoomConfig, void>("/update", roomConfig);
  }
}