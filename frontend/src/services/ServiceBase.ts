import messageBus from "@/utils/MessageBus";
import axios, { AxiosError, AxiosInstance } from "axios";

export default class ServiceBase {
  private httpClient: AxiosInstance;

  protected constructor(path: string) {
    this.httpClient = axios.create({
      baseURL: `/api${path}`,
      withCredentials: true,
    });
  }

  protected async post<TIn, TOut>(path: string, data?: TIn | undefined, contentType?: string): Promise<TOut> {
    const config = contentType ? {
      headers: {
        "Content-Type": contentType,
      },
    } : undefined;
    
    try {
      const res = await this.httpClient.post(path, data, config);
      return res.data;
    } catch (e) {
      if (e instanceof AxiosError) {
        const error = e as AxiosError;

        if (error.status === 500) {
          console.error(error);
          messageBus.error(error.message);
        }
      }

      throw e;
    }
  }

  protected async get<TIn, TOut>(path: string, params?: TIn | undefined, contentType?: string): Promise<TOut> {
    const config = contentType ? {
      headers: {
        "Content-Type": contentType,
      },
    } : undefined;
    
    try {
      const res = await this.httpClient.get(path, {
        params,
      });
      return res.data;
    } catch (e) {
      if (e instanceof AxiosError) {
        const error = e as AxiosError;

        if (error.status === 500) {
          console.error(error);
          messageBus.error(error.message);
        }
      }

      throw e;
    }
  }
}