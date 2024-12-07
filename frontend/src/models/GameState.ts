import GameStateType from "./GameStateType";
import QuizQuestion from "./quiz/QuizQuestion";

export default interface GameState {
  type: GameStateType;
  question?: QuizQuestion;
  answer?: string;
}