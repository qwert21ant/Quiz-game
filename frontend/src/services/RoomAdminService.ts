import RoomConfig from "@/models/RoomConfig";
import AuthServiceBase from "./AuthServiceBase";
import RoomState from "@/models/RoomState";
import KickParticipantDTO from "@/models/KickParticipantDTO";

export default class RoomAdminService extends AuthServiceBase {
  public constructor() {
    super("/room/admin");
  }

  public async getConfig(): Promise<RoomConfig> {
    return this.get("/config");
  }

  public async updateConfig(roomConfig: RoomConfig): Promise<void> {
    return this.post("/update", roomConfig);
  }

  public async getState(): Promise<RoomState> {
    return this.get("/state");
  }

  public async openRoom(): Promise<void> {
    return this.post("/open");
  }

  public async closeRoom(): Promise<void> {
    return this.post("/close");
  }

  public async kickParticipant(data: KickParticipantDTO): Promise<void> {
    return this.post("/kick", data);
  }
}