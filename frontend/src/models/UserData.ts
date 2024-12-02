import QuizPreview from "./QuizPreview";

export default interface UserData {
  username: string;
  activeRoom?: string;
  quizes: QuizPreview[];
}