export default interface RoomState {
  isOpen: boolean;
  isGameRunning: boolean;
  id?: string;
  participants: string[];
}