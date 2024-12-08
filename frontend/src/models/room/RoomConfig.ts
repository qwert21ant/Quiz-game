import RoomInfo from "./RoomInfo";

export default interface RoomConfig {
  info: RoomInfo;
  quizId?: string;
  maxParticipants: number;
}