import AuthServiceBase from "./AuthServiceBase";
import RoomState from "@/models/RoomState";
import RoomIdDTO from "@/models/RoomIdDTO";
import RoomPublicInfo from "@/models/RoomPublicInfo";

export default class RoomParticipantService extends AuthServiceBase {
  public constructor() {
    super("/room/participant");
  }

  public async getInfo(data: RoomIdDTO): Promise<RoomPublicInfo> {
    return this.get("/info", data);
  }

  public async getState(data: RoomIdDTO): Promise<RoomState> {
    return this.get("/state", data);
  }

  public async joinRoom(data: RoomIdDTO): Promise<void> {
    return this.post("/join", data);
  }

  public async leaveRoom(data: RoomIdDTO): Promise<void> {
    return this.post("/leave", data);
  }
}