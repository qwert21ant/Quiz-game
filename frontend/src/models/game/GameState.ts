import GameStateType from "./GameStateType";
import GameQuestion from "./GameQuestion";
import GameAnswer from "./GameAnswer";
import GameResults from "./GameResults";

export default interface GameState {
  type: GameStateType;
  question?: GameQuestion;
  answer?: GameAnswer;
  results?: GameResults;
  nextQuestionInd?: number;
  participantsAnswers?: Record<string, GameAnswer>;
}