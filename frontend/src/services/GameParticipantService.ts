import RoomConfig from "@/models/room/RoomConfig";
import CommonServiceBase from "./CommonServiceBase";
import GameAnswer from "@/models/game/GameAnswer";
import GameState from "@/models/game/GameState";

export default class GameParticipantService extends CommonServiceBase {
  public constructor() {
    super("/game/participant");
  }

  public async answer(roomId: string, answer: GameAnswer): Promise<RoomConfig> {
    return this.post("/answer", { roomId, answer});
  }

  public async getState(roomId: string): Promise<GameState> {
    return this.get("/state", { value: roomId });
  }
}