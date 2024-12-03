export interface Message {
  type: "error" | "warning" | "info",
  content: string;
}

export class MessageBus {
  private listeners: ((msg: Message) => void)[] = [];

  private emit(msg: Message) {
    this.listeners.forEach(listener => listener(msg));
  }

  public error(content: string) {
    this.emit({ type: "error", content });
  }

  public warning(content: string) {
    this.emit({ type: "warning", content });
  }

  public info(content: string) {
    this.emit({ type: "info", content });
  }

  public on(listener: (msg: Message) => void) {
    this.listeners.push(listener);
  }
}

const messageBus = new MessageBus();

export default messageBus;