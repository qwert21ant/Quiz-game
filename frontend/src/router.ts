import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import Login from "./views/Auth.vue";
import Home from "./views/Home.vue";
import Dashboard from "./views/dashboard/Dashboard.vue";
import Room from "./views/room/Room.vue";
import GameParticipant from "./views/game/GameParticipant.vue";
import GameAdmin from "./views/game/GameAdmin.vue";
import Quiz from "./views/quiz/Quiz.vue";

const routes: RouteRecordRaw[] = [
  { path: "/auth", component: Login },
  { path: "/", component: Home },
  { path: "/dashboard", component: Dashboard },
  { path: "/quiz/:id", component: Quiz },
  { path: "/room", component: Room },
  { path: "/game", component: GameAdmin },
  { path: "/game/:id", component: GameParticipant },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;