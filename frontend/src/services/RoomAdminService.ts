import RoomConfig from "@/models/room/RoomConfig";
import CommonServiceBase from "./CommonServiceBase";
import RoomState from "@/models/room/RoomState";

export default class RoomAdminService extends CommonServiceBase {
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

  public async kickParticipant(user: string): Promise<void> {
    return this.post("/kick", { value: user });
  }

  public async startGame(): Promise<void> {
    return this.post("/start");
  }
}