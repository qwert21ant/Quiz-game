<template>
  <div class="ma-auto" v-if="!isInitialized">
    <v-progress-circular indeterminate size="x-large"/>
  </div>
  <div v-else>
    <v-container max-width="1000px">
      <v-card>
        <v-card-title class="d-flex">
          <div>Комната {{ roomInfo.name }}</div>
          <v-btn
            class="ml-auto"
            color="error"
            variant="outlined"
            prepend-icon="mdi-close"
            text="Покинуть"
            @click="leaveRoom"
          />
        </v-card-title>
        <v-card-text>
          {{ roomInfo.description ?? "Без описания" }}
        </v-card-text>
      </v-card>
    </v-container>
    <v-container max-width="1000px">
      <div
        v-if="gameState.type == GameStateType.Waiting"
        class="d-flex flex-column align-center"
      >
        <v-progress-circular indeterminate size="x-large"/>
        <div class="mt-3">Ожидание других участников</div>
      </div>
      <div v-if="gameState.type == GameStateType.Question"></div>
      <div v-if="gameState.type == GameStateType.Answer"></div>
      <div v-if="gameState.type == GameStateType.Results"></div>
    </v-container>
  </div>
</template>

<script lang="ts">
import GameState from '@/models/GameState';
import GameStateType from '@/models/GameStateType';
import RoomInfo from '@/models/room/RoomInfo';
import router from '@/router';
import RoomParticipantService from '@/services/RoomParticipantService';
import { defineComponent } from 'vue';

export default defineComponent({
  name: "GameParticipant",
  data() {
    return {
      GameStateType,
      roomService: new RoomParticipantService(),
      roomInfo: null as RoomInfo,
      gameState: null as GameState,

      isInitialized: false as boolean,

      stateRefreshTimer: null,
    };
  },
  methods: {
    async leaveRoom() {
      await this.roomService.leaveRoom(this.$route.params.id);

      router.push({ path: "/dashboard" });
    },
    async refreshState() {
      this.gameState = await this.roomService.getState(this.$route.params.id);
    },
  },
  async mounted() {
    this.roomInfo = await this.roomService.getInfo(this.$route.params.id);
    await this.refreshState();

    this.isInitialized = true;

    this.stateRefreshTimer = setInterval(() => this.refreshState(), 2000);
  },
  beforeUnmount() {
    if (this.stateRefreshTimer)
      clearInterval(this.stateRefreshTimer);
  },
});
</script>