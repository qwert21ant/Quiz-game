import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import Login from "./views/Auth.vue";
import Home from "./views/Home.vue";
import Dashboard from "./views/dashboard/Dashboard.vue";
import Room from "./views/room/Room.vue";

const routes: RouteRecordRaw[] = [
  { path: "/auth", component: Login },
  { path: "/", component: Home },
  { path: "/dashboard", component: Dashboard },
  { path: "/room", component: Room },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;