export class ErrorBus {
  private listeners: ((err: string) => void)[] = [];

  public emit(err: string) {
    this.listeners.forEach(listener => listener(err));
  }

  public on(listener: (err: string) => void) {
    this.listeners.push(listener);
  }
}

const errorBus = new ErrorBus();

export default errorBus;