import messageBus from "@/utils/MessageBus";
import ServiceBase from "./ServiceBase";
import router from "@/router";
import { AxiosError } from "axios";

export default class CommonServiceBase extends ServiceBase {
  protected constructor(path: string) {
    super(path);
  }

  protected override async post<TIn, TOut>(path: string, data?: TIn | undefined, contentType?: string): Promise<TOut> {
    try {
      return await super.post<TIn, TOut>(path, data, contentType);
    } catch (e) {
      if (e instanceof AxiosError) {
        const error = e as AxiosError;

        if (error.status === 401) {
          messageBus.error("Unauthorized");
          router.push({ path: "/auth" });
        } else if (error.status === 400) {
          let msg = error.message;
          if (error.response.data)
            msg += ". " + error.response.data;
          messageBus.error(msg);
        }
      }

      throw e;
    }
  }

  protected override async get<TIn, TOut>(path: string, params?: TIn | undefined, contentType?: string): Promise<TOut> {
    try {
      return await super.get<TIn, TOut>(path, params, contentType);
    } catch (e) {
      if (e instanceof AxiosError) {
        const error = e as AxiosError;

        if (error.status === 401) {
          messageBus.error("Unauthorized");
          router.push({ path: "/auth" });
        } else if (error.status === 400) {
          let msg = error.message;
          if (error.response.data)
            msg += ". " + error.response.data;
          messageBus.error(msg);
        }
      }

      throw e;
    }
  }
}