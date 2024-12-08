import AuthServiceBase from "./AuthServiceBase";
import GameState from "@/models/game/GameState";

export default class GameAdminService extends AuthServiceBase {
  public constructor() {
    super("/game/admin");
  }

  public async startGame(): Promise<void> {
    return this.post("/start");
  }

  public async endGame(): Promise<void> {
    return this.post("/end");
  }

  public async selectNextQuestion(questionInd: number): Promise<void> {
    return this.post("/selectNextQuestion", { value: questionInd });
  }

  public async nextQuestion(): Promise<void> {
    return this.post("/nextQuestion");
  }

  public async goToResults(): Promise<void> {
    return this.post("/gotoResults");
  }

  public async getState(): Promise<GameState> {
    return this.get("/state");
  }
}