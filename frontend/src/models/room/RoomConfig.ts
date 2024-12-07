import RoomInfo from "./RoomInfo";

export default interface RoomConfig {
  info: RoomInfo;
  quizName?: string;
  maxParticipants: number;
}