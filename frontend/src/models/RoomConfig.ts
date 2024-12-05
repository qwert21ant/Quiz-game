import RoomPublicInfo from "./RoomPublicInfo";

export default interface RoomConfig {
  info: RoomPublicInfo;
  quizName?: string;
  maxParticipants: number;
}