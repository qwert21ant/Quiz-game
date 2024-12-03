import RequestResult from "@/models/RequestResult";
import messageBus from "@/utils/MessageBus";
import axios, { AxiosError, AxiosInstance } from "axios";

export default class ServiceBase {
  private httpClient: AxiosInstance;

  protected constructor(path: string) {
    this.httpClient = axios.create({
      baseURL: `http://localhost:5099/api${path}`,
      withCredentials: true,
    });
  }

  protected async post<TIn, TOut>(path: string, data?: TIn | undefined): Promise<RequestResult<TOut>> {
    try {
      const res = await this.httpClient.post(path, data);

      return {
        status: res.status,
        data: res.data,
      };
    } catch (e) {
      if (e instanceof AxiosError) {
        const error = e as AxiosError;

        if (error.status === 500) {
          console.error(error);
          messageBus.error(error.message);
          throw e;
        }

        return {
          status: error.response.status,
          data: error.response.data as TOut,
          error: e,
        };
      } else
        throw e;
    }
  }

  protected async get<TIn, TOut>(path: string, params?: TIn | undefined): Promise<RequestResult<TOut>> {
    try {
      const res = await this.httpClient.get(path, {
        params,
      });

      return {
        status: res.status,
        data: res.data,
      };
    } catch (e) {
      if (e instanceof AxiosError) {
        const error = e as AxiosError;

        if (error.status === 500) {
          console.error(error);
          messageBus.error(error.message);
          throw e;
        }

        return {
          status: error.response.status,
          data: error.response.data as TOut,
          error: e,
        };
      } else
        throw e;
    }
  }
}