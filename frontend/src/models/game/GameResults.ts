export default interface GameResults {
  score?: number;
  place?: number;
  leaderboard?: Record<string, number>;
}