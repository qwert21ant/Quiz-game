import RoomConfig from "@/models/RoomConfig";
import AuthServiceBase from "./AuthServiceBase";
import RoomState from "@/models/RoomState";
import RoomJoinData from "@/models/RoomJoinData";
import KickParticipantData from "@/models/KickParticipantData";

export default class RoomService extends AuthServiceBase {
  public constructor() {
    super("/room");
  }

  public async getConfig(): Promise<RoomConfig> {
    return this.get<void, RoomConfig>("/config");
  }

  public async updateConfig(roomConfig: RoomConfig): Promise<void> {
    return this.post("/update", roomConfig);
  }

  public async getState(): Promise<RoomState> {
    return this.get<void, RoomState>("/state");
  }

  public async openRoom(): Promise<void> {
    return this.post("/open");
  }

  public async closeRoom(): Promise<void> {
    return this.post("/close");
  }

  public async kickParticipant(data: KickParticipantData): Promise<void> {
    return this.post("/kick", data);
  }

  public async joinRoom(data: RoomJoinData): Promise<void> {
    return this.post("/join", data);
  }

  public async leaveRoom(data: RoomJoinData): Promise<void> {
    return this.post("/leave", data);
  }
}