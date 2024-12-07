import AuthServiceBase from "./AuthServiceBase";
import RoomInfo from "@/models/room/RoomInfo";
import GameState from "@/models/GameState";

export default class RoomParticipantService extends AuthServiceBase {
  public constructor() {
    super("/room/participant");
  }

  public async joinRoom(roomId: string): Promise<void> {
    return this.post("/join", { id: roomId });
  }

  public async leaveRoom(roomId: string): Promise<void> {
    return this.post("/leave", { id: roomId });
  }

  public async getInfo(roomId: string): Promise<RoomInfo> {
    return this.get("/info", { id: roomId });
  }

  public async getState(roomId: string): Promise<GameState> {
    return this.get("/state", { id: roomId });
  }
}