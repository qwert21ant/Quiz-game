import AuthServiceBase from "./AuthServiceBase";
import RoomInfo from "@/models/room/RoomInfo";

export default class RoomParticipantService extends AuthServiceBase {
  public constructor() {
    super("/room/participant");
  }

  public async joinRoom(roomId: string): Promise<void> {
    return this.post("/join", { value: roomId });
  }

  public async leaveRoom(roomId: string): Promise<void> {
    return this.post("/leave", { value: roomId });
  }

  public async getInfo(roomId: string): Promise<RoomInfo> {
    return this.get("/info", { value: roomId });
  }

  public async getIsGameRunning(roomId: string): Promise<boolean> {
    return !!(await this.get("/isGameRunning", { value: roomId }) as any).value;
  }
}