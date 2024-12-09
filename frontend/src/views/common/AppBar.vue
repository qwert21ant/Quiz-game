<template>
  <v-app-bar height="60" elevation="3">
    <v-app-bar-title>
      <a
        style="text-decoration: none; color: black; cursor: pointer;"
        @click="goToDashboard"
      >
        Quiz game
      </a>
    </v-app-bar-title>
    <template #append v-if="username">
      <v-btn class="fill-height">
        <v-icon
          class="mr-2"
          icon="mdi-account-circle"
          size="x-large"
        />
        <div>{{ username }}</div>

        <v-menu activator="parent">
          <v-list>
            <v-list-item
              class="text-error"
              @click="logout"
            >
            <v-icon
              class="mr-2"
              icon="mdi-logout"
            />
            <span>LOGOUT</span>
            </v-list-item>
          </v-list>
        </v-menu>
      </v-btn>
    </template>
  </v-app-bar>

  <div style="height: 60px"></div>
</template>

<script lang="ts">
import router from '@/router';
import AuthService from '@/services/AuthService';
import messageBus from '@/utils/MessageBus';
import { defineComponent } from 'vue';

export default defineComponent({
  name: "AppBar",
  data() {
    return {
      authService: new AuthService(),
      username: null as string,
    };
  },
  methods: {
    goToDashboard() {
      router.push({ path: "/dashboard" });
    },
    async logout() {
      await (new AuthService()).logout();
      messageBus.info("Logged out");
      router.push({ path: "/auth" });
    },
  },
  mounted() {
    router.beforeEach(async (to, from, next) => {
      if (to.path === "/auth" || from.path === "/auth" || from.path === "/")
        this.username = await this.authService.me();

      if (to.path === "/auth" && this.username) {
        messageBus.info("Authorized");
        next({ path: "/dashboard" });
        return;
      }

      next();
    });
  },
});
</script>
