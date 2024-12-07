import RoomConfig from "./RoomConfig";
import RoomState from "./RoomState";

export default interface Room {
  config: RoomConfig;
  state: RoomState;
}