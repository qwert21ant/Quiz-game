import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import Login from "./views/Auth.vue";
import Home from "./views/Home.vue";
import Dashboard from "./views/Dashboard.vue";

const routes: RouteRecordRaw[] = [
  { path: "/auth", component: Login },
  { path: "/", component: Home },
  { path: "/dashboard", component: Dashboard },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;