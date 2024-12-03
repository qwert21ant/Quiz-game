import RequestResult from "@/models/RequestResult";
import messageBus from "@/utils/MessageBus";
import ServiceBase from "./ServiceBase";
import router from "@/router";

export default class AuthServiceBase extends ServiceBase {
  protected constructor(path: string) {
    super(path);
  }

  protected override async post<TIn, TOut>(path: string, data?: TIn | undefined): Promise<RequestResult<TOut>> {
    const res = await super.post<TIn, TOut>(path, data);

    if (res.status === 401) {
      messageBus.error("Unauthorized");
      router.push({ path: "/auth" });
      throw res.error;
    }

    return res;
  }

  protected override async get<TIn, TOut>(path: string, params?: TIn | undefined): Promise<RequestResult<TOut>> {
    const res = await super.get<TIn, TOut>(path, params);

    if (res.status === 401) {
      messageBus.error("Unauthorized");
      router.push({ path: "/auth" });
      throw res.error;
    }

    return res;
  }
}