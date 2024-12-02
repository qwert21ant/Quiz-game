export default interface RequestResult<T> {
  status: number;
  data?: T;
  error?: Error;
}