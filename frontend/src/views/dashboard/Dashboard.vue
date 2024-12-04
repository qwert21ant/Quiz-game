<template>
  <div class="ma-auto" v-if="!userData">
    <v-progress-circular indeterminate size="x-large"/>
  </div>
  <div v-else>
    <AppBar :username="userData.username" />

    <v-container max-width="1000px">
      <v-card>
        <v-card-title>Управление комнатой</v-card-title>
        <v-card-text>
          <v-btn
            class="mr-3"
            prepend-icon="mdi-door"
            text="Моя комната"
            @click="enterMyRoom"
          />
          <v-btn
            prepend-icon="mdi-location-enter"
            color="secondary"
            text="Войти по коду"
            @click="showRoomEntranceDialog = true"
          />
        </v-card-text>
      </v-card>
    </v-container>
    <v-container max-width="1000px">
      <v-card>
        <v-card-title class="d-flex justify-space-between">
          <div>
            Мои квизы
          </div>
          <v-btn
            prepend-icon="mdi-plus"
            @click="createQuiz"
          >
            Создать
          </v-btn>
        </v-card-title>
        <v-card-text>
          <div v-if="!userData.quizes.length">
            У вас нет квизов
          </div>
          <div v-else class="d-flex flex-wrap">
            <v-card
              v-for="quiz in userData.quizes"
              class="ma-1"
              width="250"
              @click="openQuiz(quiz.name)"
            >
              <v-card-title>{{ quiz.name }}</v-card-title>
              <v-card-text>{{ quiz.name }}</v-card-text>
            </v-card>
          </div>
        </v-card-text>
      </v-card>
    </v-container>
    <RoomEntranceDialog
      v-model="showRoomEntranceDialog"
      @join="joinRoom"
    />
  </div>
</template>

<script lang="ts">
import UserData from '@/models/UserData';
import router from '@/router';
import AuthService from '@/services/AuthService';
import UserService from '@/services/UserService';
import messageBus from '@/utils/MessageBus';
import { defineComponent } from 'vue';
import AppBar from '../common/AppBar.vue';
import RoomEntranceDialog from './RoomEntranceDialog.vue';
import RoomService from '@/services/RoomService';

export default defineComponent({
  name: "Dashboard",
  components: { AppBar, RoomEntranceDialog },
  data() {
    return {
      authService: new AuthService(),
      userService: new UserService(),
      roomService: new RoomService(),

      userData: null as UserData,

      showRoomEntranceDialog: false as boolean,
    };
  },
  methods: {
    async logout() {
      await this.authService.logout();
      messageBus.info("Logged out");
      router.push({ path: "/auth" });
    },
    enterMyRoom() {
      router.push({ path: "/room" });
    },
    async joinRoom(roomId: string) {
      await this.roomService.joinRoom({
        id: roomId,
      });
    },
    createQuiz() {

    },
    openQuiz(name: string) {

    },
  },
  async mounted() {
    this.userData = await this.userService.getUserData();
  },
});
</script>
