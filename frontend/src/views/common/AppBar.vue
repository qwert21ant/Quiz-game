<template>
  <v-app-bar height="60" elevation="3">
    <v-app-bar-title>Quiz game</v-app-bar-title>
    <template #append>
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
  props: {
    username: String,
  },
  methods: {
    async logout() {
      await (new AuthService()).logout();
      messageBus.info("Logged out");
      router.push({ path: "/auth" });
    },
  },
});
</script>
